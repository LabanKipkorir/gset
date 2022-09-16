import { BreakpointObserver } from '@angular/cdk/layout';
import { AfterViewInit, Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { NewAssessmentDialogComponent } from '../../dialogs/new-assessment-dialog/new-assessment-dialog.component';
import { AssessmentService } from '../../services/assessment.service';
import { GalleryService } from '../../services/gallery.service';
import FuzzySearch from 'fuzzy-search';

@Component({
  selector: 'app-search-page',
  templateUrl: './search-page.component.html',
  styleUrls: ['./search-page.component.scss']
})
export class SearchPageComponent implements OnInit, AfterViewInit  {

  hoverIndex = -1;  
  searchQuery: string ='';  
  show:boolean = false;
  
  saveOldgalleryData: any[];
  //TODO: need to set the galleryItems from the query filter
  //page should start out with everything
  
  galleryItems: any[] = [];
  galleryItemsTmp: any[] = [];
  searcher: any;

  constructor(public dialog:MatDialog, 
    public breakpointObserver: BreakpointObserver, 
    public gallerySvc: GalleryService, 
    public assessSvc: AssessmentService) { 
  }


  ngOnInit(): void {
    this.gallerySvc.getGalleryItems("CSET").subscribe(
      (resp: any) => {        
        resp.rows.forEach(element => {
           element.galleryItems.forEach( item => {
            this.galleryItems.push(item);
            this.galleryItemsTmp.push(item);
           })           
          }
        );        
        this.searcher = new FuzzySearch(this.galleryItems, ['title', 'description'], {
          caseSensitive: false,
          sort: false
        });
      }
    );
   
    this.checkNavigation();
  }
  ngAfterViewInit(): void {
    this.checkNavigation();
  }
  checkNavigation(){
    let swiperPrev = document.getElementsByClassName('swiper-button-prev');
    let swiperNext = document.getElementsByClassName('swiper-button-next');
    if(window.innerWidth < 620){
     
      if(swiperPrev != null && swiperNext != null){
        for(var i = 0; i < swiperPrev.length; i++){
          swiperPrev[i].setAttribute('style', 'display:none');
          swiperNext[i].setAttribute('style','display:none');
        }
      }
    } else {
      if(swiperPrev != null && swiperNext != null){
        for(var i = 0; i < swiperPrev.length; i++){
          swiperPrev[i].removeAttribute('style');
          swiperNext[i].removeAttribute('style');
        }
      }
    }
  }

 

  showButtons(show: boolean){
    this.show=show;
  }

  onSlideChange(){}

  sort(searchQuery:string){        
    //this.galleryItemsTmp  = this.galleryItems.filter(x=> x.title.includes(searchQuery));    
    this.galleryItemsTmp = this.searcher.search(searchQuery);
    console.log(this.galleryItemsTmp);
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
