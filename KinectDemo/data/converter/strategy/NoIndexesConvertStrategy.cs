
namespace KinectDemo
{
    abstract class NoIndexesConvertStrategy : ConvertStrategy
    {
        public override bool DoIndexesMatter() => false;
    }
}
