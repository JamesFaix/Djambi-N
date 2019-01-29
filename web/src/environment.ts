export default class Environment {
    static apiAddress() : string {
        return process.env.API_URL + "/api";
    }
}