# План реализации мокапа анкеты

## Принципы

- **Desktop-first**: целевой viewport — 1440px, мобильная адаптация не планируется
- Порядок: layout/визуал → структура компонентов → контент/домен
- Каждый этап компилируется и ничего не ломает
- Старые CSS-классы не удалять — они используются в других страницах

---

## Этап 0 — Отдельный layout для анкеты

> **Критично для десктопа.** Сейчас страница использует `AntroShell` (с шапкой и навигацией).  
> Мокап — полноэкранный опыт без шапки, фон заполняет весь viewport (1440px).

**Новый файл `QuestionnaireLayout.razor`** в `Antro.Web/Layout/`:

```razor
@inherits LayoutComponentBase

<div class="q-layout">
    @Body
</div>
```

**CSS для `q-layout`:**
- `min-height: 100vh`
- `background: var(--color-background)`
- Радиальный градиент через `::before` (как в мокапе — два пятна в верхних углах), `position: fixed`, `pointer-events: none`

**В `Questionnaire.razor`** добавить первой строкой:

```razor
@layout QuestionnaireLayout
```

**Почему важно для десктопа:** без этого шапка `AntroShell` съедает верхнее пространство, sticky прогресс-бар позиционируется неверно, а фоновые градиенты не заполняют viewport.

---

## Этап 1 — CSS (`app.css`)

Добавляем новые классы, не трогая старые.

### Фон и layout страницы

| Класс / элемент | Назначение |
|---|---|
| `.q-layout::before` | `position: fixed; inset: 0` — два радиальных градиента; `pointer-events: none; z-index: 0` |
| `.q-progress` | `position: sticky; top: 0; z-index: 30; height: 4px` — полная ширина viewport |
| `.q-progress-fill` | Градиент terra → terra-deep; `transition: width 0.55s cubic-bezier(.2,.7,.2,1)` |

### Topbar

| Класс | Назначение |
|---|---|
| `.questionnaire-topbar` | `max-width: 560px; margin: 0 auto; padding: 18px 24px 0` — центрирован по контенту |
| `.icon-btn` | `36×36px`, `border-radius: 10px`, прозрачный фон, hover → лёгкий тинт |
| `.step-counter` | `12.5px`, `font-weight: 700`, `color: var(--color-faint-text)` |
| `.q-skip` | Прозрачная кнопка, `font-size: 13.5px`; модификатор `.q-skip--hidden` → `visibility: hidden` |

### Shell и анимация

| Класс | Назначение |
|---|---|
| `.questionnaire-shell` | `max-width: 560px; margin: 0 auto; padding: 32px 24px 80px; position: relative; z-index: 1` |
| `.questionnaire-mark` | Flex, по центру, `margin-bottom: 32px` — точка + «antro» |
| `.q-screen` | `animation: q-fadein .4s cubic-bezier(.2,.7,.2,1)` |
| `@keyframes q-fadein` | `from { opacity: 0; transform: translateY(8px) }` → `to { opacity: 1; transform: translateY(0) }` |

### Заголовок вопроса

| Класс | Назначение |
|---|---|
| `.q-title` | `26px`, `font-weight: 800`, `letter-spacing: -.015em`, `line-height: 1.2` |
| `.q-sub` | `15px`, `color: var(--color-soft-text)`, `max-width: 440px` |
| `.q-sub b` | `color: var(--color-terracotta-deep)`, `font-weight: 700` |

### Карточки опций

| Класс | Назначение |
|---|---|
| `.opts` | `display: flex; flex-direction: column; gap: 12px` |
| `.opts.opts--grid` | `display: grid; grid-template-columns: 1fr 1fr; gap: 12px` — для вопроса о занятости |
| `.opt` | `background: var(--color-card); border: 2px solid transparent; border-radius: 14px; box-shadow: var(--shadow-card); padding: 18px 20px; min-height: 64px; display: flex; align-items: center; gap: 16px; cursor: pointer; width: 100%` |
| `.opt:hover` | `background: var(--color-card-hover); box-shadow: var(--shadow-soft)` |
| `.opt:active` | `transform: translateY(1px)` |
| `.opt.opt--picked` | `background: terra-tint; border-color: var(--color-terracotta); box-shadow: 0 0 0 4px rgba(185,103,79,.12), var(--shadow-soft)` |
| `.opt-ic` | `42×42px; border-radius: 12px; background: sand; color: terra-deep; display: grid; place-items: center; flex-shrink: 0; font-weight: 800; font-size: 18px` |
| `.opt.opt--picked .opt-ic` | `background: var(--color-terracotta); color: #fff` |
| `.opt-body` | `min-width: 0; flex: 1` |
| `.opt-label` | `16px; font-weight: 700; letter-spacing: -.005em` |
| `.opt-note` | `13px; color: var(--color-faint-text); margin-top: 4px` |
| `.opt-note .good` | `color: var(--color-olive)` |
| `.opt-note .bad` | `color: #A06250` |
| `.opts--grid .opt` | `flex-direction: column; align-items: center; text-align: center; padding: 22px 14px; min-height: 120px; gap: 10px` |
| `.opts--grid .opt-ic` | `width: 48px; height: 48px` |

### Поля даты

| Класс | Назначение |
|---|---|
| `.date-row` | `display: grid; grid-template-columns: 1fr 1.4fr 1fr; gap: 12px; max-width: 380px` |
| `.date-field` | `background: var(--color-card); border: 2px solid rgba(61,43,31,.14); border-radius: 14px; padding: 10px 14px 12px` |
| `.date-field:focus-within` | `border-color: var(--color-terracotta); background: #fff; box-shadow: 0 0 0 4px rgba(185,103,79,.12)` |
| `.date-label` | `display: block; font-size: 11.5px; font-weight: 700; letter-spacing: .06em; text-transform: uppercase; color: var(--color-faint-text)` |
| `.date-input` | `width: 100%; border: 0; background: transparent; font: inherit; font-size: 22px; font-weight: 700; font-variant-numeric: tabular-nums; caret-color: var(--color-terracotta)` |
| `.pdr-toggle` | `display: inline-flex; align-items: center; gap: 8px; font-size: 14px; color: var(--color-terracotta); font-weight: 700; background: transparent; border: 0; cursor: pointer; margin-top: 18px` |
| `.pdr-toggle .sw` | `32×18px; border-radius: 999px; background: sand-deep; position: relative` |
| `.pdr-toggle .sw::after` | `14×14px; border-radius: 50%; background: #fff; position: absolute; top: 2px; left: 2px; transition: transform .2s` |
| `.pdr-toggle.pdr-toggle--on .sw` | `background: var(--color-terracotta)` |
| `.pdr-toggle.pdr-toggle--on .sw::after` | `transform: translateX(14px)` |
| `.q-continue` | `appearance: none; border: 0; background: var(--color-terracotta); color: #fff; font: inherit; font-weight: 800; font-size: 16px; padding: 16px 20px; border-radius: 14px; width: 100%; max-width: 380px; display: inline-flex; align-items: center; justify-content: center; gap: 10px; margin-top: 40px; cursor: pointer` |
| `.q-continue[disabled]` | `opacity: .4; cursor: default; pointer-events: none` |

### Экран завершения

| Класс | Назначение |
|---|---|
| `.q-done` | `text-align: center; padding-top: 24px` |
| `.q-done-illu` | `140×140px; margin: 0 auto 28px; display: grid; place-items: center; animation: q-pop-in .55s cubic-bezier(.2,1.4,.4,1)` |
| `@keyframes q-pop-in` | `from { transform: scale(.4); opacity: 0 }` → `to { transform: scale(1); opacity: 1 }` |
| `.q-done-title` | `30px; font-weight: 800; letter-spacing: -.02em` |
| `.q-done-sub` | `16px; color: var(--color-soft-text); max-width: 380px; margin: 0 auto 32px` |
| `.q-done-preview` | `max-width: 420px; margin: 0 auto 32px; padding: 22px 26px; border-radius: 18px; background: terra-tint градиент; text-align: left` |
| `.q-done-row` | `display: flex; justify-content: space-between; align-items: baseline; padding: 11px 0; border-bottom: 1px dashed rgba(185,103,79,.22)` |
| `.q-done-row .k` | `color: var(--color-soft-text); font-weight: 600; font-size: 14px` |
| `.q-done-row .v` | `color: var(--color-terracotta-deep); font-weight: 800; font-size: 18px` |
| `.q-done-cta` | Terra-кнопка, `display: inline-flex`, `text-decoration: none`, hover → terra-deep |

---

## Этап 2 — Иконки

### Расширить `QuestionOption` в `QuestionnaireFlow.cs`

```csharp
public sealed record QuestionOption(
    string Value,
    string Label,
    string? Description = null,
    string? IconName = null,  // "rings", "house", "check" и т.д.
    string? Glyph = null);    // "1", "2" — текстовый символ вместо SVG
```

### Новый файл `QuestionIcons.cs` в `Antro.Web/Components/`

Статический класс `QuestionIcons` со словарём `IReadOnlyDictionary<string, string>`.  
Иконки из мокапа: `rings`, `pair`, `person`, `female`, `male`, `neutral`, `check`, `q`, `house`, `coins`, `nope`.

---

## Этап 3 — `ChoiceQuestion.razor`

Полная переработка разметки:

- Убрать `.question-shell__intro` / `.choice-question__options` / `.choice-chip`
- Рендерить `.opts` + карточки `.opt`
- Новый параметр `string Layout = "list"` → при `"grid"` добавляет CSS-класс `.opts--grid`
- Иконка: `Glyph` → текст; `IconName` → SVG через `(MarkupString)QuestionIcons.Get(option.IconName)`; иначе — дефолтная окружность
- Авто-переход — **не** в компоненте, организуется в `Questionnaire.razor` (см. Этап 6)

---

## Этап 4 — `DateQuestion.razor`

Полная переработка:

- Три `<input type="number">` (ДД / ММ / ГГГГ) — классы `.date-row` / `.date-field` / `.date-label` / `.date-input`
- Убрать чипы BirthDate/DueDate → заменить на `.pdr-toggle` кнопку
  - Локальное состояние `_isPdr` (bool)
  - При переключении вызывает `SelectedInputKindChanged` (`BirthDate` или `DueDate`)
- Кнопка `.q-continue` — активна только если ДД/ММ/ГГГГ в допустимых диапазонах
- При клике Continue: собирает дату → `ValueChanged.InvokeAsync(date)` + `OnContinue.InvokeAsync()`
- Новый параметр `EventCallback OnContinue`

Диапазоны валидации: день 1–31, месяц 1–12, год 2020–2030.

---

## Этап 5 — `QuestionnaireStep.razor`

Упрощение: убрать двухколоночный layout и внутренний `ProgressBar`.  
Компонент становится тонким pass-through — рендерит только `@ChildContent`.  
Весь хром (прогресс, topbar, заголовок, подзаголовок) переезжает в `Questionnaire.razor`.

> Проверить: используется ли `QuestionnaireStep` где-то ещё кроме `Questionnaire.razor`.  
> Если нет — удалить совсем.

---

## Этап 6 — `Questionnaire.razor`

Реструктуризация главной страницы:

- Добавить `@layout QuestionnaireLayout` первой строкой
- **Sticky прогресс-бар** вне `.questionnaire-shell`:
  ```html
  <div class="q-progress">
      <div class="q-progress-fill" style="width: @ProgressPct%"></div>
  </div>
  ```
  `ProgressPct = (CurrentStepIndex + 0.5) / VisibleSteps.Count * 100`
- **Topbar** — центрирован через `.questionnaire-topbar`:
  - `<button class="icon-btn" disabled="@(CurrentStepIndex == 0)">` ← SVG
  - `<span class="step-counter">Шаг @(CurrentStepIndex+1) из @VisibleSteps.Count</span>`
  - `<button class="q-skip @(CurrentStep.Required ? "q-skip--hidden" : "")">Пропустить</button>`
- **Shell** `.questionnaire-shell`:
  - `.questionnaire-mark` — точка + «antro»
  - `<div class="q-screen" @key="CurrentStepIndex">` — ключ меняется → CSS-анимация перезапускается
  - `.q-title` и `.q-sub` — из `CurrentStep.Title` и `.Prompt`
  - Далее — компонент вопроса
- **Убрать** `<section class="questionnaire-panel">` (кнопки Назад/Далее)
- **Авто-переход** в `OnChoiceSelected` после обновления Draft:
  ```csharp
  await Task.Delay(380);
  GoNextOrComplete();
  StateHasChanged();
  ```
- **Экран завершения:** поле `_showDone = true` вместо немедленного `NavigateTo`  
  Рендерим `.q-done` с SVG-иллюстрацией, статистикой и кнопкой-ссылкой на `/dashboard`
- `OnContinue` от `DateQuestion` → вызывает `GoNextOrComplete()`

---

## Этап 7 — Домен и контент

Самый рискованный этап. Затрагивает `UserProfile.cs`, `QuestionnaireFlow.cs`, `Questionnaire.razor` и `ConcreteEligibilityRules.cs`.

### Нужные изменения enum-значений

| Вопрос | Текущее | Мокап | Действие |
|---|---|---|---|
| `FamilyStatus` | `SingleParent`, `TwoParentFamily` | В браке / Не в браке оба / Один | Добавить `Married`, `Partners`; оставить `SingleParent` |
| `EmploymentStatus` | `Employed`, `SelfEmployed`, `Unemployed` | Мама / Папа / Оба / Никто | Добавить `MotherOnly`, `FatherOnly`, `Both`, `NeitherParent`; старые не удалять |
| `ParentAgeBand` | `UnderThirty`, `ThirtyToThirtyFive`, `ThirtySixOrOlder` | Оба <36 / хотя бы один 36+ / один да один нет | Добавить `BothUnderThirtySix`, `AtLeastOneThirtySixOrOlder`, `Mixed` |
| `PropertyStatus` | `DoesNotOwnHome`, `OwnsHome` | Нет / Да / Не уверены | Добавить `Unsure` |
| `IncomeBand` | Абстрактные диапазоны | Конкретные суммы ₽ | Только лейблы в flow; enum не трогать |
| `MatCapitalHistory` | 4 варианта | 2 варианта (да/нет) | Убрать лишние опции из flow; enum не трогать |

### После изменений enum

1. Обновить `QuestionnaireFlow.cs` — новые тексты вопросов, лейблы, иконки, опции
2. Обновить switch-выражения в `Questionnaire.razor` — `OnChoiceSelected`, `GetSelectedValue`, `HasAnswer`
3. Обновить `QuestionnaireFlow.BuildProfile()`
4. Проверить `ConcreteEligibilityRules.cs` — обновить условия под новые enum-значения

---

## Порядок выполнения

```
0. QuestionnaireLayout   → новый layout без шапки, фон на весь viewport
1. app.css               → новые стили, старые не трогать
2. QuestionOption        → добавить IconName / Glyph
3. QuestionIcons.cs      → создать файл со SVG-иконками
4. ChoiceQuestion        → новые карточки .opt
5. DateQuestion          → три поля + PDR-toggle + OnContinue
6. QuestionnaireStep     → упростить до pass-through
7. Questionnaire.razor   → реструктурировать + авто-переход + done-screen
8. Домен + контент       → обновить enum, flow, eligibility rules
```
