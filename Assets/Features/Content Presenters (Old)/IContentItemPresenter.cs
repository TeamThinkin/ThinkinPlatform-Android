using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public interface IContentItemPresenter
{
    Type[] DtoTypes { get; }

    GameObject GameObject { get; }

    string Id { get; }

    bool HasVisual { get; }

    CollectionContentItemDto ContentDto { get; }

    Task LoadFromDto(CollectionContentItemDto Dto, bool IsSymbolic);
}
