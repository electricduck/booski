#!/usr/bin/env bash

platforms=(
    "linux-x64"
    "linux-arm64"
    "linux-musl-x64"
    "linux-musl-arm64"
    "osx-x64"
    "osx-arm64"
    "win-x64"
    "win-arm64"
)
# SEE: https://learn.microsoft.com/en-us/dotnet/core/rid-catalog

base_dir="$(dirname "$(realpath -s "$0")")"
git_tag="$(git describe --exact-match --tags)"
git_commit="$(git rev-parse --short HEAD)"
version=""

if [[ -z $git_tag ]]; then
    version="git.$git_commit"
else    
    version="$(echo $git_tag | sed 's:.*/::')"
fi

for platform in ${platforms[@]}; do
    out_dir="$base_dir/bin/$version/$platform"
    mkdir -p "$out_dir"

    if [[ -d "$out_dir" ]]; then
        for file in "$out_dir"/*; do
            rm -rf "$file"
        done
    fi

    echo "Publishing: $version ($platform)"
    dotnet publish \
        --configuration Release \
        --output "$out_dir" \
        --runtime $platform

    if [[ $BOOSKI_PUBLISH_NO_POST_PUBLISH != 1 ]]; then
        bin_name="Booski"
        bin_ext="bin"

        if [[ $platform == win* ]]; then
            bin_name="$bin_name.exe"
            bin_ext="exe"
        fi

        mv "$out_dir/$bin_name" "$out_dir/../booski-$version-$platform.$bin_ext"
        rm -rf "$out_dir"
    fi
done