import os
import json
import shutil
import requests
import subprocess

from os import path
from zipfile import ZipFile
from csproj import parse_csproj
from pathlib import Path

INDEX_URL = "https://api.nuget.org/v3-flatcontainer/contentwarning.gamelibs.steam/index.json"
FRAMEWORK = "netstandard2.1"
UNITY_FRAMEWORK = "netstandard2.0"
FINAL_PATCH = "changes3.patch"

PATCHES = [
    "changes.patch",
    "changes2.patch",
]

DEPS = [
    "https://nuget.bepinex.dev/v3/package/unityengine.modules/2022.3.10/unityengine.modules.2022.3.10.nupkg",
    "https://www.nuget.org/api/v2/package/WebSocketSharp/1.0.3-rc11",
    "https://www.nuget.org/api/v2/package/Newtonsoft.JSON/13.0.3",
]

ASSEMBLIES = [
    "Assembly-CSharp.dll",
    "Zorro.Core.Runtime.dll",
    "Zorro.UI.Runtime.dll",
    "Zorro.Recorder.dll",
    "Zorro.Settings.Runtime.dll",
    "Zorro.PhotonUtility.dll",
]

versions = json.loads(requests.get(INDEX_URL).text)["versions"]
version = versions[-1]
url = f"https://www.nuget.org/api/v2/package/ContentWarning.GameLibs.Steam/{version}"
tmp_dir = "_tmp"
out_dir = "out"
asm_dir = path.join(tmp_dir, "assemblies")

if not path.exists(tmp_dir):
    os.makedirs(tmp_dir)

def download_pkgs():
    print("Downloading packages...")

    for i, u in enumerate([url] + DEPS):
        out = f"{i}.nupkg"
        out = path.join(tmp_dir, out)

        print(f"Download: {u}")

        with open(out, "wb") as f:
            f.write(requests.get(u).content)

def extract_assemblies():
    print("Extracting assemblies...")

    for i, _ in enumerate(DEPS + [url]):
        out = f"{i}.nupkg"
        out = path.join(tmp_dir, out)

        print(f"Extract: {out}")

        with ZipFile(out, "r") as zip:
            zip.extractall(tmp_dir)

def copy():
    print("Copying assemblies...")

    dir1 = path.join(tmp_dir, "lib", UNITY_FRAMEWORK)
    dir2 = path.join(tmp_dir, "ref", FRAMEWORK)
    dir1 = [path.join(dir1, it) for it in os.listdir(dir1)]
    dir2 = [path.join(dir2, it) for it in os.listdir(dir2)]
    files = dir1 + dir2

    for file in files:
        if not path.exists(asm_dir):
            os.makedirs(asm_dir)

        new = path.join(asm_dir, path.basename(file))

        print(f"--- {file} -> {new}")

        shutil.copyfile(file, new)

def decompile():
    print("Decompiling (stripped) assemblies...")

    if path.exists(out_dir):
        shutil.rmtree(out_dir)

    paths = [path.join(asm_dir, it) for it in ASSEMBLIES]

    subprocess.check_output(["ilspycmd", "--nested-directories", "-p", "-o", out_dir] + paths)

def add_deps():
    print("Adding dependencies...")

    with open(path.join(out_dir, "nuget.config"), "w") as f:
        f.write("""<?xml version="1.0" encoding="utf-8"?>
    <configuration>
        <packageSources>
            <add key="BepInEx" value="https://nuget.bepinex.dev/v3/index.json"/>
        </packageSources>
    </configuration>""")

    dirs = os.listdir(out_dir)
    dirs = [path.join(out_dir, it) for it in dirs]
    dirs = filter(lambda x: path.isdir(x), dirs)

    for dir in dirs:
        subprocess.check_output(["dotnet", "add", "package", "UnityEngine.Modules", "--version", "2022.3.10"], cwd=dir)
        # subprocess.check_output(["dotnet", "add", "package", "WebSocketSharp", "--version", "1.0.3-rc11"], cwd=dir)

        proj_path = path.join(dir, path.basename(dir) + ".csproj")

        with open(proj_path, "r") as f:
            raw = f.read()

        raw = raw.replace("_tmp/assemblies/", path.abspath(asm_dir) + "/")

        with open(proj_path, "w") as f:
            f.write(raw)

def patch():
    print("Patching...")

    for patch in PATCHES:
        with open(patch, "rb") as f:
            patch_data = f.read()

        proc = subprocess.Popen(["patch", "-s", "-p0"], stdin=subprocess.PIPE)
        proc.communicate(input = patch_data)
        proc.wait()

    items = list(Path(out_dir.join("Assembly-CSharp")).rglob("*.cs"))
    items = filter(lambda x: path.isfile(x), items)

    for item in items:
        with open(item, "r") as f:
            raw = f.read()

        if "namespace " in raw:
            continue

        raw = "namespace DefaultNamespace;\n\n" + raw

        with open(item, "w") as f:
            f.write(raw)

    with open(FINAL_PATCH, "rb") as f:
        patch_data = f.read()

    proc = subprocess.Popen(["patch", "-s", "-p0"], stdin=subprocess.PIPE)
    proc.communicate(input = patch_data)
    proc.wait()

    shutil.rmtree(path.join(out_dir, "Assembly-CSharp", "Photon"))
    shutil.rmtree(path.join(out_dir, "Assembly-CSharp", "UnityEngine"))
    shutil.rmtree(path.join(out_dir, "Assembly-CSharp", "UnityEditor"))
    shutil.rmtree(path.join(out_dir, "Assembly-CSharp", "Properties"))

def clean():
    print("Cleaning project files...")

    dirs = os.listdir(out_dir)
    dirs = [path.join(out_dir, it) for it in dirs]
    dirs = filter(lambda x: path.isdir(x), dirs)

    for dir in dirs:
        proj_file = path.join(dir, path.basename(dir) + ".csproj")
        xml = parse_csproj(proj_file).to_xml()

        with open(proj_file, "w") as f:
            f.write(xml)

def build():
    print("Building...")

    subprocess.check_output(["dotnet", "build", "--property", "NoWarn=\"CS0169;CS0649\""], cwd=out_dir)

def main():
    print("Extracting...")

    download_pkgs()
    extract_assemblies()
    copy()
    decompile()
    add_deps()
    patch()
    clean()
    build()

    print("Done!")

if __name__ == "__main__":
    main()
