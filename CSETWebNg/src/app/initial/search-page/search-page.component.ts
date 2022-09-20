import { BreakpointObserver, BreakpointState } from '@angular/cdk/layout';
import { AfterViewInit, Component, Input, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { NewAssessmentDialogComponent } from '../../dialogs/new-assessment-dialog/new-assessment-dialog.component';
import { AssessmentService } from '../../services/assessment.service';
import { GalleryService } from '../../services/gallery.service';
import FuzzySearch from 'fuzzy-search';
import { SwiperComponent } from 'swiper/angular';
import { SwiperOptions } from 'swiper';
import Fuse from 'fuse.js';
import { map } from 'lodash';

@Component({
  selector: 'app-search-page',
  templateUrl: './search-page.component.html',
  styleUrls: ['./search-page.component.scss']
})
export class SearchPageComponent implements OnInit, AfterViewInit  {
  @ViewChild('swiper', {static: false}) swiper?: SwiperComponent;

  @Input() searchQuery:string;
  hoverIndex = -1;  
  
  show:boolean = false;
  fuse: any;
  fuseResults: any[];
  options = {
    includeScore: true,
    keys:["title", "descriptions"]
  };
  
  saveOldgalleryData: any[];
  //TODO: need to set the galleryItems from the query filter
  //page should start out with everything
  
  galleryItems: any[] = [];
  galleryItemsTmp: any[] = [];
  searcher: any;
  cardsPerView: number = 1;
  rows=[];
  config: SwiperOptions = {
    slidesPerView: 1,
    spaceBetween: 7,
    slidesPerGroup: 1, 
    
    breakpoints: {
      200: {
        slidesPerView:1,
      },
      620:{
        slidesPerView:2,
      },
      800: {
        slidesPerView: 3, 
      },
      1220:{
        slidesPerView:4,
      },
      1460:{
        slidesPerView:5
      }
    }, 
    on: {
      resize: ()=>{
        
      }
    }
  };
  constructor(public dialog:MatDialog, 
    public breakpointObserver: BreakpointObserver, 
    public gallerySvc: GalleryService, 
    public assessSvc: AssessmentService) { 
      
  
  }

  ngAfterInit(){
   
  }

  ngOnChanges(){
    if(this.searchQuery && this.fuse){
      this.sort();
    }
  }

  ngOnInit(): void {
    
    this.breakpointObserver.observe(['(min-width:200px)', '(min-width:620px)','(min-width:800px)', '(min-width:1220px)', '(min-width:1460px)']).subscribe((state:BreakpointState)=>{
      if(state.breakpoints['(min-width:200px)']){
        this.cardsPerView=1;
      } 
      if(state.breakpoints['(min-width:620px)']){
        this.cardsPerView=2;
      } 
      if(state.breakpoints['(min-width:800px)']){
        this.cardsPerView=3;
      } 
      if(state.breakpoints['(min-width:1220px)']){
        this.cardsPerView=4;
      } 
      if(state.breakpoints['(min-width:1460px)']){
        this.cardsPerView=5;
      }
      this.shuffelCards(this.cardsPerView);
    });

    this.gallerySvc.getGalleryItems("CSET").subscribe(
      (resp: any) => {        
        resp.rows.forEach(element => {
           element.galleryItems.forEach( item => {
            this.galleryItems.push(item);
            this.galleryItemsTmp.push(item);            
           })           
          }
        );   
        this.fuse = new Fuse(this.galleryItemsTmp, this.options)
        this.galleryItemsTmp = map(this.galleryItemsTmp, (item, index)=>({
            item,
            refIndex: index,
            matches: [],
            score: 1,
          })    
        );
        this.shuffelCards(this.cardsPerView);
        this.sort();
      }
    );
    
    this.checkNavigation();
  }

  ngAfterViewInit(): void {
    this.checkNavigation();
  }

  shuffelCards(i:number){
    
    this.rows = [];
    var count=this.cardsPerView;
    var row = [];
    for(var x = 0; x < this.galleryItemsTmp.length; x++ ){
      if(count > 0){
        row.push(this.galleryItemsTmp[x])
        count--;
        if(count == 0 || x == this.galleryItemsTmp.length-1)
        {
          this.rows.push(row);
          count=this.cardsPerView;
          row=[];
        }
      }
    }
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

  sort(){        
    console.log(this.searchQuery);
    if(!this.fuse){
      this.fuse = new Fuse(this.galleryItemsTmp, this.options);
    }
    if(this.searchQuery){
      this.galleryItemsTmp = this.fuse.search(this.searchQuery);
    } else {
      this.galleryItemsTmp = map(this.galleryItemsTmp, (item, index)=>({
          item,
          refIndex: index,
          matches: [],
          score: 1,
        })    
      );
    }
    
    this.shuffelCards(this.cardsPerView);
  }

  onHover(i:number){
    this.hoverIndex = i;
  }

  onHoverOut(i:number, cardId: number){
    this.hoverIndex = i; 
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
