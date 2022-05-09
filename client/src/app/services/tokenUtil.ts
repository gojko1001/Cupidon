export function getDecodedToken(token: string){
    try{
        return JSON.parse(window.atob(token.split('.')[1]));
    } catch(e){
        return null;
    }
}

export function signOut(): void {
    localStorage.clear();
}

export function getAccessToken(): string | null {
    return localStorage.getItem("ACTKN")
}

export function setAccessToken(value: string): void {
    localStorage.setItem("ACTKN", value)
}

export function getRefreshToken(): string | null {
    return localStorage.getItem("RFTKN")
}

export function setRefreshToken(value: string): void {
    localStorage.setItem("RFTKN", value)
}