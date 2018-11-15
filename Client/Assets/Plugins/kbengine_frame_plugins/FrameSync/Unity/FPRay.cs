namespace KBEngine
{

    /**
    *  @brief Represents a ray with origin and direction. 
    **/
    public class FPRay
	{
		public TSVector direction;
		public TSVector origin;

		public FPRay (TSVector origin, TSVector direction)
		{
			this.origin = origin;
			this.direction = direction;
		}

	}
}

