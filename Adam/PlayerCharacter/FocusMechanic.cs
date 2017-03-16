using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adam.PlayerCharacter
{
    public class FocusMechanic
    {
        Player _player;

        private int _focusLevel = 0;

        public delegate void FocusHandler(Player player, int focusLevel);
        public event FocusHandler OnFocusLevelChange;

        public FocusMechanic(Player player)
        {
            _player = player;
        }

        public void AddFocus(int amount)
        {
            _focusLevel += amount;
            OnFocusLevelChange?.Invoke(_player, _focusLevel);
        }

        public void ResetFocus()
        {
            _focusLevel = 0;
            OnFocusLevelChange?.Invoke(_player, _focusLevel);
        }

        public int GetFocus()
        {
            if (_focusLevel > 100) return 100;
            else return _focusLevel;
        }
    }
}
