using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

public class PresenterFactory : MonoBehaviour
{
    [SerializeField] private PresentersScriptableObject Presenters;

    private static Dictionary<Type, GameObject> dtoToPresenterPrefab;

    public static PresenterFactory Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        dtoToPresenterPrefab = new Dictionary<Type, GameObject>();
        foreach(var item in Presenters.Presenters)
        {
            var presenter = item.GetComponent<IContentItemPresenter>();
            if(presenter != null)
            {
                foreach(var dtoType in presenter.DtoTypes)
                {
                    dtoToPresenterPrefab.Add(dtoType, item);
                }
            }
        }
    }

    public async Task<IContentItemPresenter> Instantiate(CollectionContentItemDto Dto)
    {
        IContentItemPresenter presenter = null;
        try
        {
            var dtoType = Dto.GetType();
            if (dtoToPresenterPrefab.ContainsKey(dtoType))
            {
                var presenterPrefab = dtoToPresenterPrefab[dtoType];
                var item = GameObject.Instantiate(presenterPrefab);
                presenter = item.GetComponent<IContentItemPresenter>();
                if (presenter == null)
                {
                    Debug.Log("Couldnt get presenter for item: " + dtoType.Name);
                }
                else
                {
                    await presenter.LoadFromDto(Dto);
                    presenter.GameObject.name = Dto.DisplayName + " (" + Dto.Id + ")";
                }
            }
            else Debug.Log("Unrecognized presenter type: " + Dto.MimeType);
        }
        catch(Exception ex)
        {
            Debug.LogError("Error factory instanting " + Dto.Id + " : " + Dto.DisplayName + ". " + ex.Message);
        }
        return presenter;
    }
}
