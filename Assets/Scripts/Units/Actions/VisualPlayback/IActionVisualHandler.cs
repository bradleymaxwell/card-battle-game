using System.Collections;

namespace Units.Actions.VisualPlayback
{
    public interface IActionVisualHandler
    {
        IEnumerator PlayCor(ActionPerformResult result);
    }
}