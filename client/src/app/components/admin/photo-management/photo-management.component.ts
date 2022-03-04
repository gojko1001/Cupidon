import { Component, OnInit } from '@angular/core';
import { Photo } from 'src/app/model/photo';
import { AdminService } from 'src/app/services/admin.service';

@Component({
  selector: 'app-photo-management',
  templateUrl: './photo-management.component.html',
  styleUrls: ['./photo-management.component.css']
})
export class PhotoManagementComponent implements OnInit {
  photosToApprove: Photo[] = [];

  constructor(private adminService: AdminService) { }

  ngOnInit(): void {
    this.loadPhotosForApproval();
  }

  loadPhotosForApproval(){
    this.adminService.getPhotosForApproval().subscribe((photos: Photo[]) => {
      this.photosToApprove = photos;
    })
  }

  approvePhoto(photoId: number){
    this.adminService.approvePhoto(photoId).subscribe(() => {
      this.photosToApprove.splice(this.photosToApprove.findIndex(p => p.id === photoId), 1);
    })
  }

  rejectPhoto(photoId: number) {
    this.adminService.rejectPhoto(photoId).subscribe(() => {
      this.photosToApprove.splice(this.photosToApprove.findIndex(p => p.id === photoId), 1);
    })
  }

}
