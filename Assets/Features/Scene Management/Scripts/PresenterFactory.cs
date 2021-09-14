using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

    public GameObject Instantiate(CollectionContentItemDto Dto)
    {
        var dtoType = Dto.GetType();
        if (dtoToPresenterPrefab.ContainsKey(dtoType))
        {
            var presenterPrefab = dtoToPresenterPrefab[dtoType];
            var item = GameObject.Instantiate(presenterPrefab);
            item.GetComponent<IContentItemPresenter>().SetDto(Dto);
            return item;
        }
        return null;
    }
}
