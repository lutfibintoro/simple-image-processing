namespace TresStresHold.Command
{
    internal interface ICommand
    {
        public void Go();
        public bool IsFalseCommandCheck();
        public ICommand? SetColorInformation();
    }
}
