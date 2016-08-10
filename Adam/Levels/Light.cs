namespace Adam.Levels
{
    class Light
    {
        public const int MaxIntensity = 25;
        public bool IsSource { get; set; }
        public int Strength { get; set; }
        public int Index { get; set; }

        public Light(int index)
        {
            Index = index;
        }

        public void MakeSource(int strength)
        {
            IsSource = true;
            Strength = strength;
        }

        public void Destroy(int i)
        {

        }
    }
}
