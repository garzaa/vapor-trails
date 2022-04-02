alias butler="D:/Program\ Files/butler-windows-amd64/butler.exe"
alias 7z="C:/Program\ Files/7-Zip/7z.exe"

source ./set_version.sh

release_name="r$BUILD_VERSION"

function zip() {
    for i in win-exe win32-exe osx webgl gnu-linux; do
        7z a ../demos/zips/vapor-trails-$i.zip ../demos/vapor-trails-$i 
    done
}

function mastermash() {
    git add -A
    git commit -m "release $build_num"
    git push
    git checkout release
    git merge -X theirs master
    git add -A
    git commit -m "merged in latest master"
    git push --force
    git checkout master
}

function releasezips() {
    # create the release
    curl \
        -X POST \
        -H "Authorization: token $GITHUB_TOKEN" \
        -H "Accept: application/vnd.github.v3+json" \
        https://api.github.com/repos/garzaa/vapor-trails/releases \
        -d "{\"tag_name\":\"$release_name\", \"name\": \"$BUILD_VERSION\" }"
    
    # upload files to that release
    for i in win-exe win32-exe osx webgl gnu-linux; do
        # update: this does NOT work, return early
        break
        # 7z a -tzip ../demos/zips/vapor-trails-$i.zip ../demos/vapor-trails-$i
        curl \
            -X POST \
            -H "Authorization: token $GITHUB_TOKEN" \
            -H "Accept: application/vnd.github.v3+json" \
            -H "Content-Type: application/z" \
            --data-binary @"../demos/zips/vapor-trails-$i.zip" \
            "https://uploads.github.com/repos/garzaa/vapor-trails/releases/$release_name/assets?name=vapor-trails-$i.zip"
    done
}

function gitrelease() {
    # mastermash
    releasezips
}

function itchrelease() {
    for i in win-exe win32-exe osx webgl gnu-linux; do
        butler push ../demos/vapor-trails-$i sevencrane/vapor-trails:$i
    done
}

set -x

zip
gitrelease && itchrelease

python busybox.py --build $BUILD_VERSION --release

set +x
