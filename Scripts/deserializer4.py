import json
import unitypack
import sys

with open(sys.argv[1], "rb") as f:
  bundle = unitypack.load(f)

for asset in bundle.assets:
  for id, object in asset.objects.items():
    if object.type == "AssetBundleDataObject":
      data = object.read()
      print(data["resUrl"])
      for key, value in enumerate(data["BaseAssetBundles"]):
        if value["assetBundleName"] == "asset_textlpatch":
          print(value["resname"])
          break

      #print(type(data["BaseAssetBundles"]))
      #f = open("ResData.txt", mode="w", encoding="utf-8")
      #f.write(str(data))
      #print(data["resUrl"])
      #print(data["daBaoTime"])
