export default class Color {
    constructor(
        readonly r : number,
        readonly g : number,
        readonly b : number
    ) {

    }

    toString() {
        return "(R" + this.r + ", G" + this.g + ", B" + this.b + ")";
    }

    toHex() {
        function componentToHex(c : number) : string {
            var hex = c.toString(16);
            return hex.length == 1 ? "0" + hex : hex;
        }

        return "#" + componentToHex(this.r)
            + componentToHex(this.g)
            + componentToHex(this.b);
    }

    static fromHex(hex : string) : Color {
        var match = /^#?([a-f\d]{2})([a-f\d]{2})([a-f\d]{2})$/i.exec(hex);
        if (!match) {
            return null;
        }

        return new Color(
            parseInt(match[1], 16),
            parseInt(match[2], 16),
            parseInt(match[3], 16)
        );
    }

    //#region Color effects

    multiply(other : Color) : Color {
        function multiplyComponent(a : number, b : number) {
            return Math.floor(a * b / 255);
        }

        return new Color(
            multiplyComponent(this.r, other.r),
            multiplyComponent(this.g, other.g),
            multiplyComponent(this.b, other.b)
        )
    }

    lighten(multiplier : number) : Color {
        function clamp(value : number, min : number, max : number) {
            return Math.max(min, Math.min(max, value));
        }

        multiplier = clamp(multiplier, -1, 1);
        const change = Math.round(255 * multiplier);

        return new Color(
            clamp(this.r + change, 0, 255),
            clamp(this.g + change, 0, 255),
            clamp(this.b + change, 0, 255)
        );
    }

    darken(multiplier : number) : Color {
        return this.lighten(-multiplier);
    }

    //#endregion

}