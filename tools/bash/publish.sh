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
    "net9.0"
)

#if [[ ! -z $1 ]]; then
#    platforms=($1)
#fi

base_dir="$(realpath -s "$(dirname "$(realpath -s "$0")")/../..")"
git_tag="$(git describe --exact-match --tags)"
git_commit="$(git rev-parse --short HEAD)"
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

function build {
    platform="$1"

    out_dir="$base_dir/bin/$version/$platform"
    mkdir -p "$out_dir"

    if [[ -d "$out_dir" ]]; then
        for file in "$out_dir"/*; do
            rm -rf "$file"
        done
    fi

    echo "Publishing: $version ($platform)"

    if [[ $platform == net* ]]; then
        dotnet publish \
            --configuration Release \
            --output "$out_dir" \
            -p:PublishSelfContained=false \
            -p:PublishSingleFile=false

        for runtime in "$out_dir/runtimes"/*; do
            if [[ ! " ${platforms[@]} " =~ " $(basename "${runtime}") " ]]; then
                if [[ ! -d "$runtime/lib" ]]; then
                    rm -rf "$runtime"
                fi
            fi
        done

        if [[ $BOOSKI_PUBLISH_NO_POST != 1 ]]; then
             zip_name="booski-$version-$platform"
             zip_dir="$out_dir/../$zip_name"

             rm -rf "$zip_dir"
             mkdir -p "$zip_dir"
             mv "$out_dir"/* "$zip_dir"

             (cd "$out_dir/.." && zip -r "$zip_name.zip" "$zip_name")

             rm -rf "$zip_dir"
             rm -rf "$out_dir"
         fi
    else
        dotnet publish \
            --configuration Release \
            --output "$out_dir" \
            --runtime $platform
        
        if [[ $BOOSKI_PUBLISH_NO_POST != 1 ]]; then
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
        fi
    fi
}

if [[ -z $1 ]]; then
    for platform in ${platforms[@]}; do
        build "$platform"
    done
else
    build "$1"
fi