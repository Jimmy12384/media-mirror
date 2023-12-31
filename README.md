
# Media Mirror

*Media Mirror* is a proof of concept developed during university to mirror a single
media source from a host to any number of connected clients. 

Due to time constraints of development time being around two weeks of non-dedicated 
time, the system was constrained to only dispatching music file sources from a localhost 
source. In the future, I would like to add support for new music sources like Spotify and 
Tidal. Additionally, I would like to split this tool into a micro-service to implement
clients as an embedded Internet of Things (IoT) device connected to a main server node.

## Run Locally

Clone the project

```cmd
  git clone https://github.com/Jimmy12384/media-mirror.git
```

Go to the project directory

```cmd
  cd media-mirror
```

Install dependencies

```cmd
  dotnet build -c Release
```

Start the application from the generated executable file (`MediaMirror.exe`)

```cmd
cd MediaMirror/bin/Release
media-mirror.exe
```