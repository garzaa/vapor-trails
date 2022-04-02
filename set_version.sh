export BUILD_VERSION=$(git describe --tags --match '[0–9]*' | cut -d "-" -f 1-2 | tr - .)
echo $BUILD_VERSION > version.txt
