#!/bin/bash

ps -aux | grep 'chisel-modules/chisel64' | grep -v 'grep' | grep -v 'tmux' | awk '{print $2}'
