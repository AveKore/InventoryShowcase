using UnityEngine.SceneManagement;

namespace SimpleEventBus
{
    public class SceneEventContextSubKillRule : IEventContextSubKillRule
    {
        private bool _isSceneWasChanged;
        
        public SceneEventContextSubKillRule()
        {
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }
        
        private void OnActiveSceneChanged(Scene scene0, Scene scene1)
        {
            if (!scene0.IsValid())
            {
                return;
            }
            
            _isSceneWasChanged = true;
        }
        
        public bool IsNeedToKillSubs()
        {
            if (!_isSceneWasChanged)
            {
                return false;
            }
            
            _isSceneWasChanged = false;
            return true;
            
        }
    }
}