using System;

namespace Targeting
{
    public interface ISelectable
    {
        Action OnSelect { get; set; }
        Action OnDeselect { get; set; }
    }
}