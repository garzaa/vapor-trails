#!/usr/bin/env bash

set -e
set -x
mkdir -p /root/.cache/unity3d
mkdir -p /root/.local/share/unity3d/Unity/
sh build.sh

set +x