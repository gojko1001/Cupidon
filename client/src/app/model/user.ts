import { Photo } from "./photo";

export interface User {
    username: string;
    token?: string;
    refreshToken?: string;
    profilePhotoUrl: string;
    knownAs: string;
    gender: string;
    roles: string[];
}


export interface Member {
    id: number;
    username: string;
    photoUrl: string;
    age: number;
    knownAs: string;
    created: Date;
    lastActive: Date;
    gender: string;
    introduction: string;
    lookingFor: string;
    interests: string;
    city: string;
    country: string;
    relationTo?: string;
    photos: Photo[];
}

export class UserParams {
    gender: string;
    minAge = 18;
    maxAge = 99;
    pageNumber = 1;
    pageSize = 5;
    orederBy: 'lastActive';

    constructor(user: User){
        this.gender = user.gender === 'female' ? 'male' : 'female';
    }
}