#!/usr/bin/env bash
set -euo pipefail       # e: exit on non-zero; u: fail on unset var; o pipefail: use rightmost non-zero exit
IFS=$'\n\t'

##TODO Header comment describing script purpose

ECHO="echo"
ECHO=

for p in ../../Binder/Binder/{,*/}*.cs
do
  i=${p#../../Binder/Binder/}
  echo "$i"
  sed -e '/^Globals/d;/Globals\._t/d' ../../Binder/Binder/"$i" > "$i"
done
