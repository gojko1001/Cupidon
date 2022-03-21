import { Component, Input, OnInit } from '@angular/core';
import { FileUploader } from 'ng2-file-upload';
import { take } from 'rxjs/operators';
import { Photo } from 'src/app/model/photo';
import { Member, User } from 'src/app/model/user';
import { AccountService } from 'src/app/services/account.service';
import { MembersService } from 'src/app/services/members.service';
import { getAccessToken, getRefreshToken } from 'src/app/services/tokenUtil';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit {
  @Input() member: Member;
  
  uploader: FileUploader;
  hasBaseDropzoneOver = false;
  usersUrl = environment.apiUrl + 'users/'
  user: User;

  constructor(private accountService: AccountService, private memberService: MembersService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe(response => this.user = response)
   }

  ngOnInit(): void {
    this.initilizeUploader();
  }

  setMainPhoto(photo: Photo){
    this.memberService.setMainPhoto(photo.id).subscribe(() => {
      this.user.profilePhotoUrl = photo.url;

      this.user.token = getAccessToken();
      this.user.refreshToken = getRefreshToken();
      this.accountService.setCurrentUser(this.user);
      
      this.member.photoUrl = photo.url;
      this.member.photos.forEach(p => {
        if(p.isMain)
          p.isMain = false;
        if(p.id === photo.id)
          p.isMain = true;
      })
    })
  }

  deletePhoto(photoId: number){
    this.memberService.deletePhoto(photoId).subscribe(() => {
      this.member.photos = this.member.photos.filter(p => p.id !== photoId);
    })
  }

  fileOverBase(e: any){
    this.hasBaseDropzoneOver = e;
  }

  initilizeUploader(){
    this.uploader = new FileUploader({
      url: this.usersUrl + 'add-photo',
      authToken: 'Bearer ' + getAccessToken(),
      isHTML5: true,
      allowedFileType: ['image'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 10 * 1024 * 1024
    });

    this.uploader.onAfterAddingFile = (file) => {
      file.withCredentials = false;
    }

    this.uploader.onSuccessItem = (item, response, status, headers) => {
      if(response){
        const photo: Photo = JSON.parse(response);
        this.member.photos.push(photo);
        if(photo.isMain){
          this.user.profilePhotoUrl = photo.url;
          this.member.photoUrl = photo.url;
          this.accountService.setCurrentUser(this.user);
        }
      }
    }
  }

}
