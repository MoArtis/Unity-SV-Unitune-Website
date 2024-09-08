Order: 1
Title: Create UnituneAssets 
---

A **UnituneAsset** is simply a module file encapsulated in a **scriptableAsset** for convenience.

To create a **UnituneAsset**, add a module file in your assets folder and the importer will internally create an asset for you. 
And like most other unity asset types, you can modify your module file and its **UnituneAsset** counterpart will be updated as well.

A successfully imported module file will look like this:

<?# Figure Src="/img/documentation/create-unitune-asset-imported-module.webp" Class="text-center" /?>

For Module formats that are already natively supported by Unity, one extra step is required. You have to change its importer from **AudioClip** to **UnituneAsset** like so:

<?# Figure Src="/img/documentation/create-unitune-asset-importer-selection.webp" Class="text-center" /?>

Upon use, the module data will be processed by **Libopenmpt** and then converted by the **UnituneSource** component into an **AudioClip**.

As such, the importer doesn't feature any setting. However, it does feature a **preview section** where a specific subsong can be selected and listened to.
The selected subsong in the importer's **preview section** has no impact on how the **UnituneAsset** will sound at runtimes.

The runtime playback settings are managed by the **UnituneSource** component. Refer to the [Use UnituneSource](xref:Use UnituneSource) section to get more information about its various settings.