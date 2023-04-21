# GFDecompress

* DataUtil.cs: various data conversion classes
* JsonUtil.cs: converts to JSON format
* StcBinaryReader.cs: decrypts files
* Downloader.cs: downloads JP/EN/KR/CN files
* Program.cs: main program

The .format files in the STCFormat folder contain the names for each name-value pair in the JSON. These may need to be updated over time with client updates. [girlsfrontline-deobfuscator](https://github.com/neko-gg/girlsfrontline-deobfuscator) may be a useful reference though it has not worked with recent client updates. Also see [gfl-data-miner-python](https://github.com/gf-data-tools/gfl-data-miner-python) as they have a much better idea of what they're doing then I do.
