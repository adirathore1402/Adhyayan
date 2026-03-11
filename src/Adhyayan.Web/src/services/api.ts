const API_BASE = '/api';

function getToken(): string | null {
  return localStorage.getItem('adhyayan_token');
}

function authHeaders(): HeadersInit {
  const token = getToken();
  return token ? { Authorization: `Bearer ${token}`, 'Content-Type': 'application/json' } : { 'Content-Type': 'application/json' };
}

async function handleResponse<T>(response: Response): Promise<T> {
  if (!response.ok) {
    const error = await response.json().catch(() => ({ message: 'Request failed' }));
    throw new Error(error.message ?? `HTTP ${response.status}`);
  }
  return response.json();
}

// ---------- Auth ----------
export async function register(data: { email: string; password: string; name: string; phone: string }) {
  const res = await fetch(`${API_BASE}/auth/register`, { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(data) });
  return handleResponse<{ token: string; name: string; email: string; parentId: number }>(res);
}

export async function login(data: { email: string; password: string }) {
  const res = await fetch(`${API_BASE}/auth/login`, { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(data) });
  return handleResponse<{ token: string; name: string; email: string; parentId: number }>(res);
}

// ---------- Curriculum ----------
export interface Board { id: number; code: string; name: string; description: string; isPrimary: boolean; }
export interface Grade { id: number; gradeNumber: number; displayName: string; }
export interface Subject { id: number; code: string; name: string; }
export interface Topic { id: number; name: string; description: string; sortOrder: number; }
export interface Chapter { id: number; chapterNumber: number; name: string; description: string; topics: Topic[]; }

export async function getBoards(): Promise<Board[]> {
  const res = await fetch(`${API_BASE}/curriculum/boards`);
  return handleResponse(res);
}

export async function getGrades(boardId: number): Promise<Grade[]> {
  const res = await fetch(`${API_BASE}/curriculum/boards/${boardId}/grades`);
  return handleResponse(res);
}

export async function getSubjects(): Promise<Subject[]> {
  const res = await fetch(`${API_BASE}/curriculum/subjects`);
  return handleResponse(res);
}

export async function getChapters(boardId: number, gradeId: number, subjectId: number): Promise<Chapter[]> {
  const res = await fetch(`${API_BASE}/curriculum/chapters?boardId=${boardId}&gradeId=${gradeId}&subjectId=${subjectId}`);
  return handleResponse(res);
}

// ---------- Questions ----------
export interface Question {
  id: number;
  questionText: string;
  questionType: string;
  options: string[];
  correctAnswer: string;
  explanation: string;
  difficulty: string;
}

export async function getQuestions(chapterId: number, difficulty?: string, count?: number): Promise<Question[]> {
  let url = `${API_BASE}/questions/chapter/${chapterId}?count=${count ?? 10}`;
  if (difficulty) url += `&difficulty=${difficulty}`;
  const res = await fetch(url);
  return handleResponse(res);
}

export async function generateQuestions(chapterId: number, difficulty = 'easy', count = 5): Promise<Question[]> {
  const res = await fetch(`${API_BASE}/questions/generate?chapterId=${chapterId}&difficulty=${difficulty}&count=${count}`, {
    method: 'POST', headers: authHeaders()
  });
  return handleResponse(res);
}

// ---------- Practice ----------
export interface SessionStart {
  sessionId: number;
  mode: string;
  questions: { id: number; questionText: string; questionType: string; options: string[]; difficulty: string }[];
}

export async function startSession(data: { childId: number; chapterId?: number; mode: string }): Promise<SessionStart> {
  const res = await fetch(`${API_BASE}/practice/start`, { method: 'POST', headers: authHeaders(), body: JSON.stringify(data) });
  return handleResponse(res);
}

export async function submitAnswer(sessionId: number, data: { questionId: number; selectedAnswer: string }) {
  const res = await fetch(`${API_BASE}/practice/${sessionId}/answer`, { method: 'POST', headers: authHeaders(), body: JSON.stringify(data) });
  return handleResponse<{ isCorrect: boolean; correctAnswer: string; explanation: string }>(res);
}

export async function completeSession(sessionId: number) {
  const res = await fetch(`${API_BASE}/practice/${sessionId}/complete`, { method: 'POST', headers: authHeaders() });
  return handleResponse<{ sessionId: number; totalQuestions: number; correctAnswers: number; accuracyPercent: number; chapterName: string }>(res);
}

// ---------- Dashboard ----------
export interface ChildInfo { id: number; name: string; gradeNumber: number; board: { id: number; code: string; name: string }; }
export interface ChapterProgress { chapterId: number; chapterName: string; subjectName: string; gradeNumber: number; totalQuestions: number; correctAnswers: number; accuracyPercent: number; }
export interface ChildProgress { childId: number; childName: string; gradeNumber: number; boardName: string; chapterProgress: ChapterProgress[]; }

export async function getChildren(): Promise<ChildInfo[]> {
  const res = await fetch(`${API_BASE}/dashboard/children`, { headers: authHeaders() });
  return handleResponse(res);
}

export async function addChild(data: { name: string; gradeNumber: number; boardId: number }) {
  const res = await fetch(`${API_BASE}/dashboard/children`, { method: 'POST', headers: authHeaders(), body: JSON.stringify(data) });
  return handleResponse<{ id: number; name: string; gradeNumber: number }>(res);
}

export async function getChildProgress(childId: number): Promise<ChildProgress> {
  const res = await fetch(`${API_BASE}/dashboard/children/${childId}/progress`, { headers: authHeaders() });
  return handleResponse(res);
}
