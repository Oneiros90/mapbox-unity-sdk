#!/bin/bash
cd "$(dirname "$0")/../sdkproject"

/Applications/Unity/2022.2.1f1/Unity.app/Contents/MacOS/Unity \
-quit \
-batchmode \
-projectPath . \
-executeMethod Mapbox.Editor.Build.Mapbox_iOS_build.BuildProject \
$(pwd)/Build