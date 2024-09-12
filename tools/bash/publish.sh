#!/usr/bin/env bash

# SEE: https://learn.microsoft.com/en-us/dotnet/core/rid-catalog
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

if [[ ! -z $1 ]]; then
    platforms=($1)
fi

base_dir="$(realpath -s "$(dirname "$(realpath -s "$0")")/../..")"
git_tag="$(git describe --exact-match --tags)"
git_commit="$(git rev-parse --short HEAD)"
web_dir_prefix="$(realpath -s ~/Mount/fi02~ducky/www)"
version=""

cd "$base_dir/src/Booski"

dotnet build

if [[ $? != 0 ]]; then
    exit $?
fi

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

        out_filename="booski-$version-$platform.$bin_ext"
        new_out_path="$(realpath -s "$out_dir/../$out_filename")"

        mv "$out_dir/$bin_name" "$new_out_path"
        rm -rf "$out_dir"

        if [[ -d "$web_dir_prefix" ]]; then
            web_dir="$web_dir_prefix/apps/booski/$version"

            if [[ ! -d "$web_dir" ]]; then
                mkdir -p "$web_dir"
            fi

            cp "$new_out_path" "$web_dir/$out_filename"
        fi
    fi
done