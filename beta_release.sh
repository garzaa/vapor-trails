set -x

for i in win-exe win32-exe osx webgl gnu-linux; do
    butler push ../demos/vapor-trails-$i sevencrane/vapor-trails-beta:$i
done

set +x