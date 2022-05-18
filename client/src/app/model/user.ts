import { Photo } from "./photo";

export interface User {
    username: string;
    token?: string;
    refreshToken?: string;
    profilePhotoUrl: string;
    knownAs: string;
    gender: string;
    roles: string[];
    publicActivity: boolean;
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
    publicActivity?: boolean;
    dateOfBirth?: Date;
    photos: Photo[];
}

export class UserParams {
    searchString: string;
    gender: string = 'all';
    minAge = 18;
    maxAge = 99;
    orederBy: string;
    pageNumber = 1;
    pageSize = 5;

    constructor(user?: User){
        if(user)
            this.gender = user.gender === 'female' ? 'male' : 'female';
    }
}