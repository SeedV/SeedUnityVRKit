# SeedUnityVRKit

This is a toolkit using Mediapipe to run motion tracking in Unity. 

## Setup

The recommended Unity editor version is 2021.3.0f1 LTS, but other editor may likely work.

Check out the project files into a local directory and open Unity editor. Then based on the platform, please rebuild [MediaPipeUnityPlugin](https://github.com/homuler/MediaPipeUnityPlugin) following [this instruction](https://github.com/homuler/MediaPipeUnityPlugin/wiki/Installation-Guide).

Copy the built files into [`Packages/com.github.homuler.mediapipe/Runtime/Plugins/`](https://github.com/SeedV/SeedUnityVRKit/tree/main/Packages/com.github.homuler.mediapipe/Runtime/Plugins).

## Build command

```shell
python3 build.py build --android arm64 --linkopt=-s --desktop cpu -vv
```
