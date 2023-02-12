#!/usr/bin/env bash
SCRIPTPATH="$( cd "$(dirname "$0")" ; pwd -P )"
set -eu

################################################################################
## CONFIG
################################################################################

## The plugin's name.
PLUGIN_NAME="TSIF"

## The Unity Editor to use for exporting.
BIN_UNITY='C:\Program Files\Unity\Hub\Editor\2021.3.18f1\Editor\Unity.exe'

## Directory where the exported .unitypackage file should be written to.
DIR_EXPORTS="$SCRIPTPATH/../Exports"

## The root directory of the Unity project.
DIR_PROJECT_ROOT="$SCRIPTPATH/../Unity-TSIF_Demo/"

## Path to the plugin to be exported. This path needs to be relative to the Unity project's root directory.
DIR_PLUGIN_TO_EXPORT="Assets/ThreeStudio/IPFS"



################################################################################
## Main
################################################################################

cd "$DIR_PROJECT_ROOT"

BIN_JQ="$(command -v jq)"
FILE_PLUGIN_PACKAGE_JSON="$DIR_PROJECT_ROOT/$DIR_PLUGIN_TO_EXPORT/package.json"
PLUGIN_VERSION="$(cat "$FILE_PLUGIN_PACKAGE_JSON" | "$BIN_JQ" -r '.version')"
PLUGIN_UNITY_VERSION="$(cat "$FILE_PLUGIN_PACKAGE_JSON" | "$BIN_JQ" -r '.unity')"
COMMIT_ID="$(git rev-parse --short=10 HEAD)"
FILE_EXPORT_UNITYPACKAGE="${PLUGIN_NAME}_Plugin-Source-${PLUGIN_VERSION}-Unity_${PLUGIN_UNITY_VERSION}-${COMMIT_ID}.unitypackage"

IFS=$'\n'
ARGS=()
ARGS+=( '-batchmode' )
ARGS+=( '-quit' )
ARGS+=( '-exportPackage' )
ARGS+=( "$DIR_PLUGIN_TO_EXPORT" )
ARGS+=( "$DIR_EXPORTS/$FILE_EXPORT_UNITYPACKAGE" )

echo -e "Exporting plugin as .unitypackage ..."
echo -e "  \033[0;32;1m*\033[0m Plugin Name ............. : \033[0;33;1m${PLUGIN_NAME}\033[0m"
echo -e "  \033[0;32;1m*\033[0m Plugin Version .......... : \033[0;33;1m${PLUGIN_VERSION}\033[0m"
echo -e "  \033[0;32;1m*\033[0m Plugin Unity Version .... : \033[0;33;1m${PLUGIN_UNITY_VERSION}\033[0m"
echo -e "  \033[0;32;1m*\033[0m Commit ID ............... : \033[0;33;1m${COMMIT_ID}\033[0m"
echo -e "  \033[0;32;1m*\033[0m Asset Source Directory .. : \033[0;33;1m${DIR_PLUGIN_TO_EXPORT}\033[0m"
echo -e "  \033[0;32;1m*\033[0m Exporting .unitypackage to: \033[0;33;1m$DIR_EXPORTS/$FILE_EXPORT_UNITYPACKAGE\033[0m"
echo

[[ -e "$DIR_EXPORTS" ]] || mkdir "$DIR_EXPORTS"
$BIN_UNITY ${ARGS[@]}
rc=$?

exit $rc



