using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class ImagePresenter : MonoBehaviour, IContentItemPresenter
{
    [SerializeField] private Renderer ImageRenderer;
    [SerializeField] private Transform Visual;


    private static Dictionary<string, Task> pendingImageRequests = new Dictionary<string, Task>();
    private static Dictionary<string, Material> imageMaterials = new Dictionary<string, Material>();
    private static Material baseImageMaterial;

    private Type[] dtoTypes = { typeof(ImageContentItemDto) };
    public Type[] DtoTypes => dtoTypes;
    public GameObject GameObject => gameObject;

    private ImageContentItemDto dto;


    public string Id => dto?.Id;

    public bool HasVisual { get; private set; }

    public CollectionContentItemDto ContentDto => dto;


    public async Task LoadFromDto(CollectionContentItemDto Dto, bool IsSymbolic)
    {
        dto = Dto as ImageContentItemDto;

        var material = await GetImageMaterial(dto);
        if (material != null && gameObject != null) //Bail out if the object has been destroyed while we were waiting to retrieve the image)
        {
            //var size = getWidthScaledSize(material.mainTexture.width, material.mainTexture.height, 1);
            var size = getHeightScaledSize(material.mainTexture.width, material.mainTexture.height, 1);
            ImageRenderer.sharedMaterial = material;
            Visual.localScale = new Vector3(size.x, size.y, 1);
            HasVisual = true;
        }
    }


    public static async Task<Material> GetImageMaterial(ImageContentItemDto itemData)
    {
        if (baseImageMaterial == null) baseImageMaterial = Resources.Load<Material>("Unlit Image");

        Material material = null;
        if (imageMaterials.ContainsKey(itemData.Id))
        {
            if (pendingImageRequests.ContainsKey(itemData.Id))
                await pendingImageRequests[itemData.Id];
            material = imageMaterials[itemData.Id];
        }
        else
        {
            material = Instantiate(baseImageMaterial);
            imageMaterials.Add(itemData.Id, material);

            using (var request = UnityWebRequestTexture.GetTexture(itemData.Url))
            {
                var task = request.SendWebRequest().GetTask();
                pendingImageRequests.Add(itemData.Id, task);
                await task;
                var texture = DownloadHandlerTexture.GetContent(request);
                material.mainTexture = texture;
                pendingImageRequests.Remove(itemData.Id);
            }
        }
        return material;
    }

    private Vector2 getWidthScaledSize(float originalWidth, float originalHeight, float targetWidth)
    {
        // w / h = W / H
        // h * W / w = H
        return new Vector2(targetWidth, originalHeight * targetWidth / originalWidth);
    }

    private Vector2 getHeightScaledSize(float originalWidth, float originalHeight, float targetHeight)
    {
        // w / h = W / H
        // h * W / w = H
        return new Vector2(originalWidth * targetHeight / originalHeight, targetHeight);
    }
}
