////////////////////////////////
//
//   Copyright 2022 Battelle Energy Alliance, LLC
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in all
//  copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  SOFTWARE.
//
////////////////////////////////
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Answer, Question, Option, InconsistentOption } from '../../../../../models/questions.model';
import { CisService } from '../../../../../services/cis.service';
import { ConfigService } from '../../../../../services/config.service';
import { QuestionsService } from '../../../../../services/questions.service';
import { Utilities } from '../../../../../services/utilities.service';

@Component({
  selector: 'app-option-block-nested',
  templateUrl: './option-block-nested.component.html'
})
export class OptionBlockNestedComponent implements OnInit {

  @Input() q: Question;
  @Input() opts: Option[];

  optRadio: Option[];
  optCheckbox: Option[];
  optOther: Option[];

  optionGroupName = '';
  sectionId = 0;

  handsetPortrait = false;

  // temporary debug aids
  showIdTag = this.configSvc.showQuestionAndRequirementIDs();
  showWeightTag = false;

  constructor(
    public questionsSvc: QuestionsService,
    public cisSvc: CisService,
    private utilSvc: Utilities,
    private configSvc: ConfigService,
    private route: ActivatedRoute,
    public boSvc: BreakpointObserver
  ) {

  }

  /**
   *
   */
  ngOnInit(): void {
    this.boSvc.observe(Breakpoints.HandsetPortrait).subscribe(hp => {
      this.handsetPortrait = hp.matches;
    });

    this.sectionId = +this.route.snapshot.params['sec'];
    // break up the options so that we can group radio buttons in a mixed bag of options
    this.optRadio = this.opts?.filter(x => x.optionType == 'radio');
    this.optCheckbox = this.opts?.filter(x => x.optionType == 'checkbox');
    this.optOther = this.opts?.filter(x => x.optionType != 'radio' && x.optionType != 'checkbox');

    // create a random 'name' that can be used to group the radios in this block
    this.optionGroupName = this.utilSvc.makeId(8);

    // Show integrity check warnings on page load.
    this.performIntegrityCheck();
  }

  /**
   * Returns a boolean indiating if all of the
   * options are unselected.
   */
  noneChecked(opts: Option[]) {
    let n = opts.every(o => !o.selected);
    return n;
  }

  /**
   *
   */
  changeRadio(o: Option, event): void {
    o.selected = event.target.checked;
    var answers = [];

    // add this option to the request
    answers.push(this.makeAnswer(o));

    // get the descendants for my peer radios to clean them up
    var siblingOptions = this.q.options.filter(x => x.optionId !== o.optionId);
    siblingOptions.forEach(option => {
      option.selected = false;
    });

    const descendants = this.getDescendants(siblingOptions);

    descendants.forEach(desc => {
      for (let key in desc) {
        //options are where the radio & checkboxes live within the "desc" data structure
        if (key === "options" && desc[key] != null && desc[key].length > 0) {
          var lengthOfOptions = desc[key].length;
          for (var i = 0; i <= lengthOfOptions; i++) {
            if (desc[key]["" + i + ""] != undefined) {
              desc[key]["" + i + ""].selected = false;
            }
          }
        }

        if (key === "answerText" && desc[key] != null) {
          desc[key] = '';
        }

        if (key === "answerMemo" && desc[key] != null) {
          desc[key] = '';
        }

        if (key === "freeResponseAnswer" && desc[key] != null) {
          desc[key] = '';
        }
      }
      const ans = this.makeAnswer(desc);
      answers.push(ans);
    });

    this.storeAnswers(answers, this.sectionId);
  }

  /**
   *
   */
  changeCheckbox(o: Option, event, listOfOptions): void {
    o.selected = event.target.checked;
    var answers = [];

    //don't love the super nested if's but the amount
    //of redesign to work around it is crazy so sorry
    //for all the if's
    if (!o.selected) {
      o.freeResponseAnswer = '';
    }
    else {
      if (o.isNone) {
        listOfOptions.forEach(obj => {
          if (o != obj) {
            obj.selected = false;
            answers.push(this.makeAnswer(obj));
          }
        });
      }
      else {
        listOfOptions.forEach(obj => {
          if (obj.isNone) {
            obj.selected = false;
            answers.push(this.makeAnswer(obj));
          }
        });
      }
    }


    // add this option to the request
    answers.push(this.makeAnswer(o));

    // if unselected, clean up my kids
    if (!o.selected) {
      // get the descendants for my peer radios to clean them up
      var siblingOptions = this.q.options.filter(x => x.optionId !== o.optionId);
      const descendants = this.getDescendants(siblingOptions);

      descendants.forEach(desc => {
        desc.selected = false;
        desc.answerText = '';
        desc.freeResponseAnswer = '';

        const ans = this.makeAnswer(desc);
        answers.push(ans);
      });
    }

    this.storeAnswers(answers, this.sectionId);
  }

  /**
   *
   */
  changeText(o: Option, event): void {
    o.freeResponseAnswer = event.target.value;
    const ans = this.makeAnswer(o);
    this.storeAnswers([ans], this.sectionId);
  }


  /**
   * Creates a 'clean' (unanswered) option
   */
  makeAnswer(o: Option): Answer {
    const answer: Answer = {
      answerId: o.answerId,
      questionId: o.questionId,
      questionType: 'Maturity',
      optionId: o.optionId,
      optionType: o.optionType,
      is_Maturity: true,
      is_Component: false,
      is_Requirement: false,
      questionNumber: '',
      answerText: o.selected ? 'S' : '', // options are marked 'S' for 'selected'
      freeResponseAnswer: o.freeResponseAnswer,
      altAnswerText: '',
      comment: '',
      feedback: '',
      markForReview: false,
      reviewed: false,
      componentGuid: '00000000-0000-0000-0000-000000000000'
    };

    return answer;
  }

  /**
   *
   */
  storeAnswers(answers, sectionId) {
    this.performIntegrityCheck();
    this.cisSvc.storeAnswers(answers, sectionId).subscribe((x: any) => {
      let score = x.groupingScore;
      this.cisSvc.changeScore(score);
    });
  }

  /**
   * Returns a list of all followups and options that descend
   * from the supplied options/questions.
   */
  getDescendants(y: any[]): any[] {
    const allYDescendants = []; // all descendants of all "y" objects

    if (!y || y.length === 0) {
      return allYDescendants;
    }

    y.forEach(x => {
      const xDescendants = [];
      xDescendants.push(...x.followups ?? []);
      xDescendants.push(...x.options ?? []);

      if (xDescendants.length > 0) {
        xDescendants.push(...this.getDescendants(xDescendants) ?? []);
      }

      allYDescendants.push(...xDescendants);
    });

    return allYDescendants;
  }

  /**
   * Ignores spacebar event if the target
   * is within a "div-shield" parent.
   */
  catchSpace(e: Event, tag: string) {
    let el = document.getElementById(tag);
    let foundEl = el.closest('.div-shield');
    if (foundEl) {
      e.preventDefault();
    }
  }

  /**
  * Performs an integrity check on a  question.
  */
  performIntegrityCheck() {
    const failedIntegrityCheckOptions = [];
    this.q.options.forEach((o: Option) => {
      const integrityCheckOption = this.cisSvc.integrityCheckOptions.find(option => option.optionId === o.optionId);

      if (integrityCheckOption) {
        integrityCheckOption.selected = o.selected;
      }

      integrityCheckOption?.inconsistentOptions.forEach(option => {
        if (this.cisSvc.integrityCheckOptions.find(x => x.optionId === option.optionId)?.selected && integrityCheckOption.selected) {
          failedIntegrityCheckOptions.push(option);
        }
      });
    });

    this.q.failedIntegrityCheckOptions = failedIntegrityCheckOptions;
  }
}
