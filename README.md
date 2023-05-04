# Exmatter
A mock of the Exmatter data exfiltration tool for Adversary Emulation. Build in .NET Framework. Intended to be used with 'execute-assembly'.

## Setup
Both methods require the permission of `write-files`.

| DropBox
Create an App in DropBox and retrieve the access-token value.

| Slack
Create an organization and App in Slack server and retrieve the oauth token value from settings. 

## Usage

This is a .NET assembly intended to be compiled and used through a C2. You can accomplish this in Sliver with `execute-assembly` or `inline-execute-assembly`.

![exmatter-1](https://user-images.githubusercontent.com/57839593/236327562-ffb6bc26-353a-411a-ab11-b89aad7fbddb.png)
![exmatter-2](https://user-images.githubusercontent.com/57839593/236327576-5487483a-a573-4632-bae1-456007486f2b.png)


