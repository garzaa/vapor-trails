function gitrelease() {
    git add -A
    git commit -m "release commit"
    git push
    git checkout release
    git merge -X theirs master
    git add -A
    git commit -m "merged in latest master"
    git push --force
    git checkout master
}

function itchrelease() {
    for i in win-exe win32-exe osx webgl gnu-linux; do
        butler push ../demos/vapor-trails-$i sevencrane/vapor-trails:$i
    done
}

set -x

gitrelease
itchrelease

set +x