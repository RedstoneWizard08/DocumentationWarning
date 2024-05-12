from xml.etree import ElementTree as ET
from .cl import CSProj

def parse_csproj(path: str) -> CSProj:
    with open(path, "r") as d:
        raw = d.read()
    
    tree = ET.fromstring(raw)

    return CSProj.parse(tree)
