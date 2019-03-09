

namespace Core
{
    public delegate void CommonHandler();

    public delegate void BallCollisionHandler(PathFollower ball, PathFollower coll);
    public delegate void BallHandler();
}