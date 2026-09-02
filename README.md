## PC Remote Control

[![Unit tests](https://github.com/JakubKaras/PcRemoteControl/actions/workflows/unit_tests.yml/badge.svg?branch=main)](https://github.com/JakubKaras/PcRemoteControl/actions/workflows/unit_tests.yml)

Tool for interacting with devices over local network.

This app uses .NET MAUI framework to create cross-platform remote control app for devices on local network.

# Prerequisites
The devices to be controled has to have WOL enabled for the wake up to workk.
For the shutdown functionality to work, a process listening on a port specified in the `ShutdownHandler` for the shutdown message has to be setup. In the case of the default handler, that is simple `shutdown`.

[comment]: # (TODO add a simple bash script)
