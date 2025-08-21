# Fn+Esc on Asus ProArt P16

On Linux, the **Fn+Esc** key combination (used to lock/unlock the function keys) does not work out of the box.
To address this, I created a small program that allows you to toggle the lock manually.

This key combination is not currently mapped in the kernel module `hid-asus.c`.
You can verify this yourself by running:

```bash
sudo dmesg -W
```

and then pressing **Fn+Esc**.
You should see a message similar to:

``` text
asus 0003:0B05:19B6.0002: Unmapped Asus vendor usagepage code 0x4e
```

## .NET Self-contained vs Normal

Since the program runs with sudo, you may encounter issues such as dotnet: command not found.
To avoid this, I also provide a self-contained release.

If you prefer, you can still use the normal version, as long as you are comfortable configuring sudo to correctly locate the dotnet runtime.

## Tested Environment

This program was developed and tested on **Fedora 42** with the **GNOME desktop**.

## Technical Description

The program (written in **.NET 8 / C#**) writes specific values to the appropriate `hidraw` device:

* `[0x5a, 0xd0, 0x4e, 0x01]` → enable the lock
* `[0x5a, 0xd0, 0x4e, 0x00]` → disable the lock

These values were extracted from USB PCap files in this [GitLab issue](https://gitlab.com/asus-linux/asusctl/-/issues/585).

The current state is saved in `/tmp/fnesc_state`, since after each reboot the lock is always set to **OFF**.

## Installation

Because the program must run with elevated privileges, you need to allow it to run with `sudo` **without requiring a password** by adding an entry in `/etc/sudoers`.

### 1. Download

Download the binary and place it wherever you prefer, e.g.:
`/home/massimo/FnEscLock/FnEsc`

Custsomize the `fn_esc.sh` setting the correct path on PGM

``` bash
PGM=/home/massimo/FnEsceLock/FnEsc
```

### 2. Configure sudoers

Edit the sudoers file with:

```bash
sudo visudo
```

⚠️ **Be careful**: saving an invalid sudoers file can lock you out of your system.
Make sure you read the [official documentation](https://www.sudo.ws/releases/stable/) before editing.

Then, at the bottom of the file, add a line like this:

``` ini
massimo ALL=(ALL) NOPASSWD: /home/massimo/FnEscLock/FnEsc
```

This grants the user `massimo` permission to run the `FnEsc` program with `sudo` without entering a password.

## Key Mapping (GNOME)

Since the kernel does not recognize the _Fn+Esc_ key combination, you can map the program to a different shortcut, for example _Ctrl+Esc_.
The _Ctrl_ key is close to _Fn_, making it easy to switch back once _Fn+Esc_ is properly supported.

To create a shortcut in GNOME:
`Settings → Keyboard → View and Customize Shortcuts → Custom Shortcuts → Add`

For the shortcut's command, use the `fn_esc.sh` script included with the program.

This script creates the temporary state file and toggles the lock between ON and OFF.
Alternatively, you can create separate shortcuts for ON and OFF by mapping them directly to the FnEsc command, e.g.:

``` bash
sudo /home/massimo/FnEscLock/FnEsc ON
sudo /home/massimo/FnEscLock/FnEsc OFF
```

### fn_esc.sh

This script toggles the lock between ON and OFF, and writes the current state to the temporary file /tmp/fnesc_output.
You can customize the script as needed.

A commented line with notify-send is included if you want a visual notification of the current state.
