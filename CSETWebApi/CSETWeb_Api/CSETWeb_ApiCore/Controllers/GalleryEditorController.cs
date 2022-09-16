using CSETWebCore.Business.GalleryParser;
using CSETWebCore.DataLayer.Model;
using CSETWebCore.Interfaces.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CSETWebCore.Api.Controllers
{
  
    [ApiController]
    public class GalleryEditorController : ControllerBase
    {
        
        private readonly ITokenManager _token;        
        private CSETContext _context;
        private IGalleryState _stateManager;
            
        public GalleryEditorController(IGalleryState parser, ITokenManager token, CSETContext context)
        {
            _token = token;
            _context = context;
            _stateManager = parser;
        }
        [HttpPost]
        [Route("api/galleryEdit/updatePosition")]
        public IActionResult updatePosition(MoveItem moveItem)
        {
            try
            {
                if(String.IsNullOrWhiteSpace(moveItem.fromId) || string.IsNullOrWhiteSpace(moveItem.toId))
                {
                    //we are changing position of the rows. 
                    //move the item from the old index to the new index and then 
                    //update the indexes of everything below them.
                    var rows = (from r in _context.GALLERY_ROWS
                               where r.Layout_Name == moveItem.Layout_Name
                               orderby r.Row_Index
                               select r).ToList();

                    _context.GALLERY_ROWS.RemoveRange(rows);
                    _context.SaveChanges();
                    //if so then remove the old one and insert it at the new position.
                    //iterate through all the items and just reassign the row_index. 
                    var itemToMove = rows[int.Parse(moveItem.oldIndex)];
                    rows.Remove(itemToMove);
                    rows.Insert(int.Parse(moveItem.newIndex), itemToMove);
                    RenumberGroup(rows);
                    _context.GALLERY_ROWS.AddRange(rows);
                    _context.SaveChanges();
                }
                else if(moveItem.fromId == moveItem.toId)
                {
                    //the items is moved in the same group 
                    //find the group and move it
                    //get the group
                    var detailsList = _context.GALLERY_GROUP_DETAILS.Where(r => r.Group_Id == int.Parse(moveItem.fromId)).OrderBy(r=> r.Column_Index).ToList();
                    var itemToMove = detailsList[int.Parse(moveItem.oldIndex)];

                    if (int.Parse(moveItem.newIndex) < int.Parse(moveItem.oldIndex))
                    {
                        //moving up the list 
                        //remove first then insert
                        //recalc
                        detailsList.RemoveAt(int.Parse(moveItem.oldIndex));
                        detailsList.Insert(int.Parse(moveItem.newIndex), itemToMove);
                    }
                    else if (int.Parse(moveItem.newIndex) > int.Parse(moveItem.oldIndex))
                    {
                        //moving down the list
                        //insert first then move down
                        detailsList.Insert(int.Parse(moveItem.newIndex), itemToMove);
                        detailsList.RemoveAt(int.Parse(moveItem.oldIndex));
                        
                    }
                    //else moving to the same spot do nothing

                        RenumberGroup(detailsList);                   
                    _context.SaveChanges();
                }
                else
                {
                    //find the old group and remove it
                    //find the new group and insert it
                    //renumber both groups
                    var detailsOldList = _context.GALLERY_GROUP_DETAILS.Where(r => r.Group_Id == int.Parse(moveItem.fromId)).OrderBy(r => r.Column_Index).ToList();
                    var itemToMove = detailsOldList[int.Parse(moveItem.oldIndex)];
                    detailsOldList.Remove(itemToMove);
                    _context.GALLERY_GROUP_DETAILS.Remove(itemToMove);
                    RenumberGroup(detailsOldList);
                    _context.SaveChanges();
                    var detailsNewList = _context.GALLERY_GROUP_DETAILS.Where(r => r.Group_Id == int.Parse(moveItem.toId)).OrderBy(r => r.Column_Index).ToList();

                    var itemToMoveClone = new GALLERY_GROUP_DETAILS()
                    {
                        Gallery_Item_Id = itemToMove.Gallery_Item_Id,
                        Click_Count = itemToMove.Click_Count,
                        Column_Index = int.Parse(moveItem.newIndex),
                        Group_Id = int.Parse(moveItem.toId)
                    };

                    
                    detailsNewList.Insert(int.Parse(moveItem.newIndex), itemToMoveClone);
                    
                    _context.GALLERY_GROUP_DETAILS.Add(itemToMoveClone);
                    RenumberGroup(detailsNewList);
                    _context.SaveChanges();
                }

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        private void RenumberGroup(List<GALLERY_GROUP_DETAILS> detailsList)
        {
            int i = 0;
            foreach (var item in detailsList)
            {
                item.Column_Index = i++;
            }
        }

        private void RenumberGroup(List<GALLERY_ROWS> rows)
        {
            int i = 0; 
            foreach(var row in rows)
            {
                row.Row_Index = i++;
            }
        }

        [HttpPost]
        [Route("api/galleryEdit/updateItem")]
        public IActionResult updateItem([FromBody]UpdateItem item)
        {
            try
            {
                if (item.IsGroup)
                {
                    var galleryGroup = _context.GALLERY_GROUP.Where(x => x.Group_Id == item.Group_Id).FirstOrDefault();
                    if (galleryGroup == null) return BadRequest();

                    galleryGroup.Group_Title = item.Value;
                    _context.SaveChanges();
                }
                else
                {
                    var galleryItem = _context.GALLERY_ITEM.Where(x => x.Gallery_Item_Id == item.Group_Id).FirstOrDefault();
                    if(galleryItem == null) return BadRequest();

                    galleryItem.Title = item.Value;
                    _context.SaveChanges();
                }

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("api/galleryEdit/getboard")]
        public IActionResult GetBoard(string Layout_Name)
        {
            try
            {
                
                
                
                var data = from r in _context.GALLERY_ROWS
                           join g in _context.GALLERY_GROUP on r.Group_Id equals g.Group_Id
                           join d in _context.GALLERY_GROUP_DETAILS on g.Group_Id equals d.Group_Id into dd
                           from dl in dd.DefaultIfEmpty()
                           where r.Layout_Name == Layout_Name && dl ==null
                           orderby r.Row_Index
                           select new { r, g};
                var rvalue = new GalleryBoardData();


                int row = -1;
                GalleryGroup galleryGroup = null;
                foreach (var item in data)
                {
                    if (row != item.r.Row_Index)
                    {
                        rvalue.Layout_Name = item.r.Layout_Name;
                        galleryGroup = new GalleryGroup();
                        galleryGroup.Group_Title = item.g.Group_Title;
                        galleryGroup.Group_Id = item.g.Group_Id;
                        rvalue.Rows.Add(galleryGroup);
                        row = item.r.Row_Index;
                    }
                }


                var items = _stateManager.GetGalleryBoard(Layout_Name);
                foreach (var item in rvalue.Rows)
                {
                    items.Rows.Insert(0, item);
                }
                


                return Ok(items);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }

    public class MoveItem
    {
        public string Layout_Name { get; set; }
        public string fromId { get; set; }
        public string toId { get;set; }
        public string oldIndex { get; set; }
        public string newIndex { get; set; }
    }

    public class UpdateItem
    {
        public bool IsGroup { get; set; }
        public int Group_Id { get; set; }

        public string Value { get; set; }
    }
}
