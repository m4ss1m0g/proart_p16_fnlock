#!/bin/bash

PGM=/home/massimo/bin/FnEsc
STATE_FILE="/tmp/fnesc_state"
OUTPUT_PGM="/tmp/fnesc_output"

if [ ! -f "$STATE_FILE" ]; then
   STATE="ON"
else
   STATE=$(cat "$STATE_FILE")
   if [ $STATE = "ON" ]; then
      STATE="OFF"
   else
      STATE="ON"
   fi
fi

sudo "$PGM" "$STATE" > "$OUTPUT_PGM"
echo "$STATE" > "$STATE_FILE"

#notify-send --expire-time=1000 --category=presence "FnEsc" "Status $STATE"
