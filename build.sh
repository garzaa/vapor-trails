echo "removing old demo folders"
for i in win-exe win32-exe osx webgl gnu-linux; do
    rm -r ../demos/vapor-trails-$i
    rm -r ../demos/zips/vapor-trails-$i.zip
done

build_num=$(git describe --tags --match '[0–9]*' | cut -d "-" -f 1-2 | tr - .)

code "C:\Users\Adrian\AppData\Local\Unity\Editor\Editor.log"
echo "use revert file to pick up new changes"

echo "building project"
"C:\Program Files\Unity\Hub\Editor\2019.4.8f1\Editor\Unity.exe" \
    -quit \
    -batchmode \
    -executeMethod ProjectBuilder.BuildAll
    -buildNum $build_num
echo "done"

python busybox.py --build $build_num
