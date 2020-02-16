#!/bin/bash
wget "https://www.dropbox.com/sh/dh5jv94udi7qzhj/AAB4y9DCaVw0OIhpDFi5x-EDa?dl=1" -O .dropbox.tmp
unzip -u -d Assets .dropbox.tmp
rm .dropbox.tmp