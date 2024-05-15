FROM gitpod/workspace-dotnet

RUN dotnet tool install --global docfx
RUN echo 'export PATH="$PATH:$HOME/.dotnet/tools"' >> ~/.bashrc
