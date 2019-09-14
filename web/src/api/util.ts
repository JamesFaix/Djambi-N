export default class ApiUtil {
    public static destructureError(error : any) : [number, string] {
        try {
            const [statusCode, message] = error;
            return [statusCode, message];
        }
        catch {
            throw error;
        }
    }
}