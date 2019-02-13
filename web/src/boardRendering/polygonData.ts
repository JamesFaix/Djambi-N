export default class PolygonData {
	private static readonly internalAngleCache : number[] = [
		undefined,
		undefined,
		undefined,
		2.0943951023932,
		1.5707963267949,
		1.25663706143592,
		1.0471975511966,
		0.897597901025655,
		0.785398163397448,
	];

	public static internalAngle(numberOfSides : number) : number {
		if (numberOfSides < 3 || numberOfSides > 8) {
			throw "Unsupported number of sides: " + numberOfSides;
		}
		return this.internalAngleCache[numberOfSides];
	}

	private static readonly externalAngleCache : number[] = [
		undefined,
		undefined,
		undefined,
		1.0471975511966,
		1.5707963267949,
		1.88495559215388,
		2.0943951023932,
		2.24399475256414,
		2.35619449019234,
	];

	public static externalAngle(numberOfSides : number) : number {
		if (numberOfSides < 3 || numberOfSides > 8) {
			throw "Unsupported number of sides: " + numberOfSides;
		}
		return this.externalAngleCache[numberOfSides];
	}

	private static readonly radiusCache : number[] = [
		undefined,
		undefined,
		undefined,
		0.577350269189626,
		0.707106781186547,
		0.85065080835204,
		1,
		1.15238243548124,
		1.30656296487638,
	];

	public static radius(numberOfSides : number) : number {
		if (numberOfSides < 3 || numberOfSides > 8) {
			throw "Unsupported number of sides: " + numberOfSides;
		}
		return this.radiusCache[numberOfSides];
	}

	private static readonly apothemCache : number[] = [
		undefined,
		undefined,
		undefined,
		0.288675134594813,
		0.5,
		0.688190960235587,
		0.866025403784439,
		1.03826069828617,
		1.20710678118655,
	];

	public static apothem(numberOfSides : number) : number {
		if (numberOfSides < 3 || numberOfSides > 8) {
			throw "Unsupported number of sides: " + numberOfSides;
		}
		return this.apothemCache[numberOfSides];
	}

	private static readonly widthCache : number[] = [
		undefined,
		undefined,
		undefined,
		1,
		1,
		1.53884176858763,
		2,
		2.19064313376741,
		2.41421356237309,
	];

	public static width(numberOfSides : number) : number {
		if (numberOfSides < 3 || numberOfSides > 8) {
			throw "Unsupported number of sides: " + numberOfSides;
		}
		return this.widthCache[numberOfSides];
	}

	private static readonly heightCache : number[] = [
		undefined,
		undefined,
		undefined,
		0.866025403784439,
		1,
		1.53884176858763,
		1.73205080756888,
		2.19064313376741,
		2.41421356237309,
	];

	public static height(numberOfSides : number) : number {
		if (numberOfSides < 3 || numberOfSides > 8) {
			throw "Unsupported number of sides: " + numberOfSides;
		}
		return this.heightCache[numberOfSides];
	}

	private static readonly centroidXOffsetCache : number[] = [
		undefined,
		undefined,
		undefined,
		0.5,
		0.5,
		0.769420884293813,
		1,
		1.09532156688371,
		1.20710678118655,
	];

	public static centroidXOffset(numberOfSides : number) : number {
		if (numberOfSides < 3 || numberOfSides > 8) {
			throw "Unsupported number of sides: " + numberOfSides;
		}
		return this.centroidXOffsetCache[numberOfSides];
	}

	private static readonly centroidYOffsetCache : number[] = [
		undefined,
		undefined,
		undefined,
		0.577350269189626,
		0.5,
		0.85065080835204,
		0.866025403784439,
		1.15238243548124,
		1.20710678118655,
	];

	public static centroidYOffset(numberOfSides : number) : number {
		if (numberOfSides < 3 || numberOfSides > 8) {
			throw "Unsupported number of sides: " + numberOfSides;
		}
		return this.centroidYOffsetCache[numberOfSides];
	}

}
