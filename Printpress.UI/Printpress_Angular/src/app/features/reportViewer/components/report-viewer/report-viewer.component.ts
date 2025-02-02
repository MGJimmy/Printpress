import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { ReportViewerService } from '../../services/report-viewer.service';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { TableColDefinitionModel } from '../../../../shared/models/table-col-definition.model';
import { TableTemplateComponent } from '../../../../shared/components/table-template/table-template.component';

@Component({
  selector: 'app-report-viewer',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    TableTemplateComponent
  ],
  templateUrl: './report-viewer.component.html',
  styleUrl: './report-viewer.component.css'
})
export class ReportViewerComponent implements OnInit {
  firstPtData: any[] = [
    {subject: "غلاف عربي", division: 'ابتدائي', pages: 20, copies: 2000, category: 6, total: 7000},
    {subject: "غلاف فرنسي", division: 'ابتدائي', pages: 20, copies: 2000, category: 6, total: 7000},
    {subject: "غلاف عربي", division: 'روضة', pages: 20, copies: 2000, category: 6, total: 7000},
    {subject: "غلاف انجليزي", division: 'ابتدائي', pages: 20, copies: 2000, category: 6, total: 7000},
    {subject: "غلاف عربي", division: 'ابتدائي', pages: 20, copies: 2000, category: 6, total: 7000},
    {subject: "غلاف متابعة", division: 'روضة', pages: 20, copies: 2000, category: 6, total: 7000},
    {subject: "غلاف عربي", division: 'ابتدائي', pages: 20, copies: 2000, category: 6, total: 7000},
    {subject: "غلاف عربي", division: 'ابتدائي', pages: 20, copies: 2000, category: 6, total: 7000},
    {subject: "غلاف عربي", division: 'ابتدائي', pages: 20, copies: 2000, category: 6, total: 7000}
  ];
  originalSource : any[] = this.firstPtData;
  pdfUrl: SafeResourceUrl | null = null;

  columnDefsPtOne:TableColDefinitionModel[] = [
    { headerName: 'المادة', column: 'subject' },
    { headerName: ' الفرقة', column: 'division' },
    { headerName: 'الصفحات', column: 'pages' },
    { headerName: 'عدد النسخ', column: 'copies' },
    { headerName: 'الفئة', column: 'category' },
    { headerName: 'الإجمالي', column: 'total' },
  ];

  constructor(private reportViewerService: ReportViewerService, private sanitizer: DomSanitizer) {}

  ngOnInit(): void {
    this.reportViewerService.getReport().subscribe((blob :BlobPart ) => {
      debugger;
      const pdfBlob = new Blob([blob], { type: 'application/pdf' });
      const pdfUrl = URL.createObjectURL(pdfBlob);
      this.pdfUrl = this.sanitizer.bypassSecurityTrustResourceUrl(pdfUrl);
    });
  }
}
