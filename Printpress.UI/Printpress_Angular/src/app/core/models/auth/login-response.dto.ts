export interface loginResponseDto  {
    success: boolean;
    message: string;
    token: tokenData;
}

export interface tokenData  {
    token: string;
    expirationTime: string;
}