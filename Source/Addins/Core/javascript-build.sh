#!/bin/bash
#
# Usage: build.sh [outputdirectory]

####################################################
# INIT                                             #
####################################################
BASEDIR=$(dirname "$1")
OUTPUTDIR="$1"

####################################################
# CLEAN                                            #
####################################################
echo "Cleaning previous build..."
rm "$OUTPUTDIR/cgi-bin/Addins/Core/data/" -r

####################################################
# CGI-BIN                                          #
####################################################
echo "Building 'cgi-bin'..."
for file in cgi-bin/*; do
    cp -rv $file "$OUTPUTDIR/cgi-bin/"
done

####################################################
# JAVASCRIPT                                       #
####################################################
echo "Building 'javascript'..."
jsbuilder javascript.jsb "$OUTPUTDIR/cgi-bin/Addins/Core/data/js/"