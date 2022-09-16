import { BreakpointObserver } from '@angular/cdk/layout';
import { Component, Input, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { NewAssessmentDialogComponent } from '../../dialogs/new-assessment-dialog/new-assessment-dialog.component';
import { AssessmentService } from '../../services/assessment.service';
import { GalleryService } from '../../services/gallery.service';

@Component({
  selector: 'app-search-card',
  templateUrl: './search-card.component.html',
  styleUrls: ['./search-card.component.scss']
})
export class SearchCardComponent implements OnInit {
  @Input() c:any;
  @Input() i:any;

  hoverIndex = -1;  

  constructor(public dialog:MatDialog, 
    public breakpointObserver: BreakpointObserver, 
    public gallerySvc: GalleryService, 
    public assessSvc: AssessmentService) { 
      console.log("card constructor called");
  }
  ngOnInit(): void {
    console.log("card constructor called with c:"+this.c +"i:"+this.i);
  }

  onHover(i:number){
    this.hoverIndex = i;
    if(i > 0){
      var el = document.getElementById('c'+i.toString()).parentElement;      
    
    var bounding = el.getBoundingClientRect();
    
      let cardDimension = {x: bounding.x, y: bounding.y, w: bounding.width, h: bounding.height};
      let viewport = {x: 0, y: 0, w: window.innerWidth, h: window.innerHeight};
      let cardSize = cardDimension.w * cardDimension.h;
      let xOverlap = Math.max(0, Math.min(cardDimension.x + cardDimension.w, viewport.x + viewport.w) - Math.max(cardDimension.x, viewport.x))      
      let offScreen = cardDimension.w - xOverlap;      
      if(offScreen > 5){
        el.style.right = (cardDimension.w).toString() +'px';
        
      }
 
    }
  }

  onHoverOut(i:number, cardId: number){
    this.hoverIndex = i;
  
    var el = document.getElementById('c'+cardId.toString()).parentElement;
    console.log(el);
    el.style.removeProperty('right');
    
  
  
  }
  
  getImageSrc(src: string){
    let path="assets/images/cards/";
    if(src){
      return path+src;
    }
    return path+'default.png';
  }

  openDialog(data: any ){
    data.path = this.getImageSrc(data.icon_File_Name_Small);
    this.dialog.open(NewAssessmentDialogComponent, {
      panelClass: 'new-assessment-dialog-responsive',
      data:data
    });
  }
}